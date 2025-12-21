'use server'

import axios from 'axios';
import { BACKEND_BASE_URL } from "@/lib/constants";

export interface VerifyEmailState {
  error?: string;
  success: boolean;
  message?: string;
  data?: Record<string, unknown>;
}

export async function verifyEmailAction(token: string): Promise<VerifyEmailState> {
  try {
    const response = await axios.get(`${BACKEND_BASE_URL}/auth/verify-email`, {
      params: { token }
    });

    return { 
      message: 'Email verified successfully! You can now login.', 
      success: true, 
      data: response.data 
    };
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const errorMessage = error.response?.data?.detail || error.response?.data?.title || error.message || 'Failed to verify email';
      return { error: errorMessage, success: false };
    }
    const message = error instanceof Error ? error.message : 'An unexpected error occurred';
    return { error: message, success: false };
  }
}
