'use client';

import { useEffect, useState } from 'react';

export default function PermissionsPage() {
  const [permissions, setPermissions] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadPermissions();
  }, []);

  const loadPermissions = async () => {
    const token = localStorage.getItem('accessToken');
    if (!token) return;

    try {
      const response = await fetch('http://localhost:5102/api/permissions', {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (response.ok) {
        const data = await response.json();
        setPermissions(data);
      }
      setLoading(false);
    } catch (error) {
      console.error('Error loading permissions:', error);
      setLoading(false);
    }
  };

  return (
    <>
      <h2>Permissions Management</h2>
      
      <section>
        <h3>System Permissions</h3>
        {loading ? (
          <p>Loading permissions...</p>
        ) : permissions.length > 0 ? (
          <table border={1}>
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Description</th>
                <th>Resource</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {permissions.map((permission) => (
                <tr key={permission.id}>
                  <td>{permission.id}</td>
                  <td>{permission.name}</td>
                  <td>{permission.description}</td>
                  <td>{permission.resource || 'N/A'}</td>
                  <td>{permission.action || 'N/A'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p>No permissions found.</p>
        )}
      </section>
      
      <hr />
      
      <section>
        <h3>Permission Assignment</h3>
        <p>Use the Roles or Groups pages to assign permissions to roles or groups.</p>
        <ul>
          <li><a href="/management/roles">Manage Roles</a></li>
          <li><a href="/management/groups">Manage Groups</a></li>
        </ul>
      </section>
      
      <hr />
      
      <p>
        <a href="/management/dashboard">Back to Dashboard</a>
      </p>
    </>
  );
}
