'use server';

import axios from 'axios';
import { BACKEND_BASE_URL } from '@/lib/constants';
import { createSession } from '@/lib/session';
import { redirect } from 'next/navigation';

export type LoginState = {
  message?: string;
  error?: string;
  success?: boolean;
};

export async function loginAction(prevState: LoginState, formData: FormData): Promise<LoginState> {
  const userName = formData.get('userName');
  const password = formData.get('password');

  if (!BACKEND_BASE_URL) {
    return { error: 'API URL is not configured' };
  }

  try {
    const response = await axios.post(`${BACKEND_BASE_URL}/auth/login`, {
      userName,
      password,
    });

    const result = response.data;
    
    await createSession(result.accessToken, result.refreshToken);
    
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const errorMessage = error.response?.data?.detail || error.response?.data?.title || error.message || 'Login failed';
      return { error: errorMessage };
    }
    const errorMessage = error instanceof Error ? error.message : 'An unexpected error occurred';
    return { error: errorMessage };
  }
  
  redirect('/management/dashboard');
}
