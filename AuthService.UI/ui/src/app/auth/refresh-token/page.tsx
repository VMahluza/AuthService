'use client';

import { useEffect, useState, useActionState } from 'react';
import { refreshTokenAction, RefreshTokenState } from './action';
import RefreshTokenCard from '@/components/auth/RefreshTokenCard';

const initialState: RefreshTokenState = {
  message: '',
  error: '',
  success: false,
  data: undefined
};

export default function RefreshTokenPage() {
  const [refreshToken, setRefreshToken] = useState('');
  const [state, formAction, isPending] = useActionState(refreshTokenAction, initialState);

  useEffect(() => {
    const storedToken = localStorage.getItem('refreshToken');
    if (storedToken) {
      setRefreshToken(storedToken);
    }
  }, []);

  // Derive the current token from state or local state
  const currentRefreshToken = (state.success && state.data?.refreshToken) 
    ? state.data.refreshToken as string
    : refreshToken;

  useEffect(() => {
    if (state.success && state.data) {
      const accessToken = state.data.accessToken as string;
      const newRefreshToken = state.data.refreshToken as string;

      if (accessToken) {
        localStorage.setItem('accessToken', accessToken);
      }
      if (newRefreshToken) {
        localStorage.setItem('refreshToken', newRefreshToken);
      }
    }
  }, [state]);

  return (
    <RefreshTokenCard
      action={formAction}
      isPending={isPending}
      error={state.error}
      success={state.success}
      message={state.message}
      refreshToken={currentRefreshToken}
      onRefreshTokenChange={setRefreshToken}
    />
  );
}
