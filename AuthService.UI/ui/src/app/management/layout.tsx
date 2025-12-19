'use client';

import { useEffect, useState } from 'react';

export default function ManagementLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    if (!token) {
      alert('Please login to access this page');
      window.location.href = '/auth/login';
    } else {
      setIsAuthenticated(true);
    }
  }, []);

  if (!isAuthenticated) {
    return <div>Checking authentication...</div>;
  }

  return (
    <>
      <header>
        <h1>AuthService - Management Portal</h1>
        <nav>
          <a href="/management/dashboard">Dashboard</a> |
          <a href="/management/profile">Profile</a> |
          <a href="/management/roles">Roles</a> |
          <a href="/management/groups">Groups</a> |
          <a href="/management/permissions">Permissions</a> |
          <a href="/auth/logout">Logout</a>
        </nav>
      </header>
      <main>
        {children}
      </main>
      <footer>
        <hr />
        <p>&copy; 2025 AuthService - Management Portal</p>
      </footer>
    </>
  );
}
