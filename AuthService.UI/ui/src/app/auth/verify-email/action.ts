'use server'

import apiClient, { getErrorMessage } from '@/lib/api-client';

export interface VerifyEmailState {
  error?: string;
  success: boolean;
  message?: string;
  data?: Record<string, unknown>;
}

export async function verifyEmailAction(token: string): Promise<VerifyEmailState> {
  try {
    const response = await apiClient.get('/auth/verify-email', {
      params: { token }
    });

    return { 
      message: 'Email verified successfully! You can now login.', 
      success: true, 
      data: response.data 
    };
  } catch (error) {
    return { 
      error: getErrorMessage(error) || 'Failed to verify email', 
      success: false 
    };
  }
}
