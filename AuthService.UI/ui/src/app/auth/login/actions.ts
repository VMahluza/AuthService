'use server';

import { post } from '@/lib/api-client';
import { createSession } from '@/lib/session';

export type LoginState = {
  message?: string;
  error?: string;
  success?: boolean;
  tokens?: {
    accessToken: string;
    refreshToken: string;
  };
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
    return { error: result.error || 'Login failed', success: false };
  }

  // Store in server-side cookies
  await createSession(result.data.accessToken, result.data.refreshToken);
  
  // Return tokens for client-side storage
  return { 
    success: true, 
    message: 'Login successful!',
    tokens: {
      accessToken: result.data.accessToken,
      refreshToken: result.data.refreshToken,
    }
  };
}
