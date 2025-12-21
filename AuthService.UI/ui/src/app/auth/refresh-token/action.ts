'use server'

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
    const response = await fetch(`${BACKEND_BASE_URL}/auth/refresh`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken }),
    });

    const text = await response.text();
    const result = text ? JSON.parse(text) : {};

    if (!response.ok) {
        return { error: result.detail || result.title || 'Failed to refresh token', success: false };
    }

    return { message: 'Token refreshed successfully!', success: true, data: result };
  } catch (error: unknown) {
    const message = error instanceof Error ? error.message : 'An unexpected error occurred';
    return { error: message, success: false };
  }
}
