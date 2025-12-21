'use server'

import { post } from '@/lib/api-client';

export interface RefreshTokenState {
  error?: string;
  success: boolean;
  message?: string;
  data?: Record<string, unknown>;
}

export async function refreshTokenAction(prevState: RefreshTokenState, formData: FormData): Promise<RefreshTokenState> {
  const refreshToken = formData.get('refreshToken');

  const result = await post<Record<string, unknown>>('/auth/refresh', {
    refreshToken
  });

  if (!result.success) {
    return { error: result.error || 'Failed to refresh token', success: false };
  }

  return { 
    message: 'Token refreshed successfully!', 
    success: true, 
    data: result.data 
  };
}
