'use server';

import { post } from '@/lib/api-client';
import { createSession } from '@/lib/session';
import { redirect } from 'next/navigation';

export type LoginState = {
  message?: string;
  error?: string;
  success?: boolean;
};

interface LoginResponse {
  accessToken: string;
  refreshToken: string;
}

export async function loginAction(prevState: LoginState, formData: FormData): Promise<LoginState> {
  const userName = formData.get('userName');
  const password = formData.get('password');

  const result = await post<LoginResponse>('/auth/login', {
    userName,
    password,
  });

  if (!result.success || !result.data) {
    return { error: result.error || 'Login failed' };
  }

  await createSession(result.data.accessToken, result.data.refreshToken);
  redirect('/management/dashboard');
}
