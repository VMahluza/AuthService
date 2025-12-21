'use server'

import { BACKEND_BASE_URL } from "@/lib/constants";

export interface VerifyEmailState {
  error?: string;
  success: boolean;
  message?: string;
  data?: Record<string, unknown>;
}

export async function verifyEmailAction(token: string): Promise<VerifyEmailState> {
  try {
    const response = await fetch(`${BACKEND_BASE_URL}/auth/verify-email?token=${encodeURIComponent(token)}`, {
      method: 'GET',
    });

    const text = await response.text();
    const result = text ? JSON.parse(text) : {};

    if (!response.ok) {
        return { error: result.detail || result.title || 'Failed to verify email', success: false };
    }

    return { message: 'Email verified successfully! You can now login.', success: true, data: result };
  } catch (error: unknown) {
    const message = error instanceof Error ? error.message : 'An unexpected error occurred';
    return { error: message, success: false };
  }
}
