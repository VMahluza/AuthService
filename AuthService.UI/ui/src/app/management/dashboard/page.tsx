'use client';

import { useEffect, useState } from 'react';

export default function DashboardPage() {
  const [userName, setUserName] = useState('');
  const [accessToken, setAccessToken] = useState('');

  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      setAccessToken(token);
      // Decode JWT to get username (simplified)
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        setUserName(payload.sub || payload.userName || 'User');
      } catch (e) {
        setUserName('User');
      }
    }
  }, []);

  return (
    <>
      <h2>Dashboard</h2>
      <p>Welcome, {userName}!</p>
      
      <hr />
      
      <section>
        <h3>Quick Actions</h3>
        <ul>
          <li><a href="/management/profile">View Profile</a></li>
          <li><a href="/management/roles">Manage Roles</a></li>
          <li><a href="/management/groups">Manage Groups</a></li>
          <li><a href="/management/permissions">Manage Permissions</a></li>
        </ul>
      </section>
      
      <hr />
      
      <section>
        <h3>Account Management</h3>
        <ul>
          <li><a href="/auth/refresh-token">Refresh Token</a></li>
          <li><a href="/auth/logout">Logout</a></li>
        </ul>
      </section>
      
      <hr />
      
      <section>
        <h3>Current Session</h3>
        <details>
          <summary>View Access Token</summary>
          <pre>{accessToken}</pre>
        </details>
      </section>
    </>
  );
}
