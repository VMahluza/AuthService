'use client';

import { useEffect, useState } from 'react';

export default function LogoutPage() {
  const [jwtToken, setJwtToken] = useState('');

  useEffect(() => {
    const token = localStorage.getItem('accessToken') || '';
    setJwtToken(token);
  }, []);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const data = {
      jwtToken: formData.get('jwtToken'),
      revokeAllSessions: formData.get('revokeAllSessions') === 'on',
    };

    try {
      const response = await fetch('http://localhost:5102/api/auth/logout', {
        method: 'DELETE',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });

      const result = await response.json();
      document.getElementById('response')!.textContent = JSON.stringify(result, null, 2);

      if (response.ok) {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        alert('Logged out successfully!');
        window.location.href = '/';
      }
    } catch (error: any) {
      document.getElementById('response')!.textContent = 'Error: ' + error.message;
    }
  };

  return (
    <>
      <h2>Logout</h2>
      <p>Sign out from your account.</p>
      
      <form onSubmit={handleSubmit}>
        <fieldset>
          <legend>Logout Options</legend>
          
          <label htmlFor="jwtToken">JWT Token:</label>
          <input 
            type="text" 
            id="jwtToken" 
            name="jwtToken" 
            defaultValue={jwtToken}
            required 
          />
          <br /><br />
          
          <label htmlFor="revokeAllSessions">
            <input 
              type="checkbox" 
              id="revokeAllSessions" 
              name="revokeAllSessions" 
            />
            Revoke all sessions
          </label>
          <br /><br />
          
          <button type="submit">Logout</button>
        </fieldset>
      </form>

      <p>
        <a href="/management/dashboard">Back to Dashboard</a>
      </p>

      <hr />
      <h3>Response</h3>
      <pre id="response"></pre>
    </>
  );
}
