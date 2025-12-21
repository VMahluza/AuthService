'use server'

import axios from 'axios';
import { BACKEND_BASE_URL } from "@/lib/constants";

export interface RefreshTokenState {
  error?: string;
  success: boolean;
  message?: string;
  data?: Record<string, unknown>;
}

export async function refreshTokenAction(prevState: RefreshTokenState, formData: FormData): Promise<RefreshTokenState> {
  const refreshToken = formData.get('refreshToken');

  try {
    const response = await axios.post(`${BACKEND_BASE_URL}/auth/refresh`, {
      refreshToken
    });

    return { 
      message: 'Token refreshed successfully!', 
      success: true, 
      data: response.data 
    };
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const errorMessage = error.response?.data?.detail || error.response?.data?.title || error.message || 'Failed to refresh token';
      return { error: errorMessage, success: false };
    }
    const message = error instanceof Error ? error.message : 'An unexpected error occurred';
    return { error: message, success: false };
  }
}
