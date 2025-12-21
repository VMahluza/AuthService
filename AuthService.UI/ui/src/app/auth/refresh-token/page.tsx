'use client';

import { useEffect, useState, useActionState } from 'react';
import { refreshTokenAction, RefreshTokenState } from './action';
import Link from 'next/link';

const initialState: RefreshTokenState = {
  message: '',
  error: '',
  success: false,
  data: undefined
};

export default function RefreshTokenPage() {
  const [refreshToken, setRefreshToken] = useState(() => localStorage.getItem('refreshToken') || '');
  const [state, formAction] = useActionState(refreshTokenAction, initialState);

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
    <>
      <h2>Refresh Access Token</h2>
      <p>Use your refresh token to get a new access token.</p>
      
      <form action={formAction}>
        <fieldset>
          <legend>Refresh Token</legend>
          
          <label htmlFor="refreshToken">Refresh Token:</label>
          <input
            type="text"
            id="refreshToken"
            name="refreshToken"
            value={currentRefreshToken}
            onChange={(e) => setRefreshToken(e.target.value)}
            required
          />
          <br /><br />
          
          <button type="submit">Refresh Token</button>
        </fieldset>
      </form>

      {state.success && (
          <p style={{ color: 'green' }}>{state.message}</p>
      )}
      {state.error && (
          <p style={{ color: 'red' }}>{state.error}</p>
      )}

      <p>
        <Link href="/auth/login">Back to Login</Link>
      </p>

      <hr />
      <h3>Response</h3>
      <pre id="response">{JSON.stringify(state, null, 2)}</pre>
    </>
  );
}
