/**
 * Client-side token management utilities
 * This file handles token storage in localStorage for client-side API calls
 */

export interface TokenPair {
  accessToken: string;
  refreshToken: string;
}

/**
 * Store tokens in localStorage (client-side only)
 */
export function setTokens(accessToken: string, refreshToken: string): void {
  if (typeof window === "undefined") return;

  localStorage.setItem("accessToken", accessToken);
  localStorage.setItem("refreshToken", refreshToken);
}

/**
 * Get access token from localStorage
 */
export function getAccessToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem("accessToken");
}

/**
 * Get refresh token from localStorage
 */
export function getRefreshToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem("refreshToken");
}

/**
 * Clear all tokens from localStorage
 */
export function clearTokens(): void {
  if (typeof window === "undefined") return;

  localStorage.removeItem("accessToken");
  localStorage.removeItem("refreshToken");
}

/**
 * Check if user has valid tokens
 */
export function hasTokens(): boolean {
  return !!(getAccessToken() && getRefreshToken());
}
