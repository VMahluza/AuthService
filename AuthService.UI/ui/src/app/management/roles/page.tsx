'use client';

import { useEffect, useState } from 'react';

export default function RolesPage() {
  const [roles, setRoles] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadRoles();
  }, []);

  const loadRoles = async () => {
    const token = localStorage.getItem('accessToken');
    if (!token) return;

    try {
      const response = await fetch('http://localhost:5102/api/roles', {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (response.ok) {
        const data = await response.json();
        setRoles(data);
      }
      setLoading(false);
    } catch (error) {
      console.error('Error loading roles:', error);
      setLoading(false);
    }
  };

  const handleCreateRole = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const token = localStorage.getItem('accessToken');

    const data = {
      name: formData.get('name'),
      description: formData.get('description'),
    };

    try {
      const response = await fetch('http://localhost:5102/api/roles', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      });

      if (response.ok) {
        alert('Role created successfully!');
        loadRoles();
        e.currentTarget.reset();
      }
    } catch (error: any) {
      alert('Error: ' + error.message);
    }
  };

  return (
    <>
      <h2>Roles Management</h2>
      
      <section>
        <h3>Create New Role</h3>
        <form onSubmit={handleCreateRole}>
          <fieldset>
            <legend>Role Information</legend>
            
            <label htmlFor="name">Role Name:</label>
            <input type="text" id="name" name="name" required />
            <br /><br />
            
            <label htmlFor="description">Description:</label>
            <textarea id="description" name="description" rows={3}></textarea>
            <br /><br />
            
            <button type="submit">Create Role</button>
          </fieldset>
        </form>
      </section>
      
      <hr />
      
      <section>
        <h3>Existing Roles</h3>
        {loading ? (
          <p>Loading roles...</p>
        ) : roles.length > 0 ? (
          <table border={1}>
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Description</th>
              </tr>
            </thead>
            <tbody>
              {roles.map((role) => (
                <tr key={role.id}>
                  <td>{role.id}</td>
                  <td>{role.name}</td>
                  <td>{role.description}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p>No roles found. Create your first role above.</p>
        )}
      </section>
      
      <hr />
      
      <p>
        <a href="/management/dashboard">Back to Dashboard</a>
      </p>
    </>
  );
}
