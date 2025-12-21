'use server';

import axios from 'axios';
import { BACKEND_BASE_URL } from '@/lib/constants';

export type LoginState = {
  message?: string;
  error?: string;
  accessToken?: string;
  refreshToken?: string;
  success?: boolean;
};

console.log('Backend Base URL:', BACKEND_BASE_URL);

export async function loginAction(prevState: LoginState, formData: FormData): Promise<LoginState> {
  const userName = formData.get('userName');
  const password = formData.get('password');

  const apiUrl = process.env.BACKEND_BASE_URL;

  console.log('API URL:', apiUrl);

  if (!apiUrl) {
    return { error: 'API URL is not configured' };
  }

  try {
    const response = await axios.post(`${apiUrl}/auth/login`, {
      userName,
      password,
    });

    const result = response.data;

    return {
      success: true,
      accessToken: result.accessToken,
      refreshToken: result.refreshToken,
      message: 'Login successful'
    };
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const errorMessage = error.response?.data?.detail || error.response?.data?.title || error.message || 'Login failed';
      return { error: errorMessage };
    }
    const errorMessage = error instanceof Error ? error.message : 'An unexpected error occurred';
    return { error: errorMessage };
  }
}
