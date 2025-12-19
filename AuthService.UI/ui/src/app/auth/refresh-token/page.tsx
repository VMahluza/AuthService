'use client';

import { useEffect, useState } from 'react';

export default function RefreshTokenPage() {
  const [refreshToken, setRefreshToken] = useState('');

  useEffect(() => {
    const token = localStorage.getItem('refreshToken') || '';
    setRefreshToken(token);
  }, []);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const data = {
      refreshToken: formData.get('refreshToken'),
    };

    try {
      const response = await fetch('http://localhost:5102/api/auth/refresh', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });

      const result = await response.json();
      document.getElementById('response')!.textContent = JSON.stringify(result, null, 2);

      if (response.ok && result.accessToken) {
        localStorage.setItem('accessToken', result.accessToken);
        localStorage.setItem('refreshToken', result.refreshToken);
        alert('Token refreshed successfully!');
      }
    } catch (error: any) {
      document.getElementById('response')!.textContent = 'Error: ' + error.message;
    }
  };

  return (
    <>
      <h2>Refresh Access Token</h2>
      <p>Use your refresh token to get a new access token.</p>
      
      <form onSubmit={handleSubmit}>
        <fieldset>
          <legend>Refresh Token</legend>
          
          <label htmlFor="refreshToken">Refresh Token:</label>
          <input 
            type="text" 
            id="refreshToken" 
            name="refreshToken" 
            defaultValue={refreshToken}
            required 
          />
          <br /><br />
          
          <button type="submit">Refresh Token</button>
        </fieldset>
      </form>

      <p>
        <a href="/auth/login">Back to Login</a>
      </p>

      <hr />
      <h3>Response</h3>
      <pre id="response"></pre>
    </>
  );
}
