'use client';

import { useEffect, useState } from 'react';

export default function GroupsPage() {
  const [groups, setGroups] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  const loadGroups = async () => {
    const token = localStorage.getItem('accessToken');
    if (!token) return;

    try {
      const response = await fetch('http://localhost:5102/api/groups', {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (response.ok) {
        const data = await response.json();
        setGroups(data);
      }
      setLoading(false);
    } catch (error) {
      console.error('Error loading groups:', error);
      setLoading(false);
    }
  };
  useEffect(() => {
    loadGroups();
  }, []);


  const handleCreateGroup = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const token = localStorage.getItem('accessToken');

    const data = {
      name: formData.get('name'),
      description: formData.get('description'),
    };

    try {
      const response = await fetch('http://localhost:5102/api/groups', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      });

      if (response.ok) {
        alert('Group created successfully!');
        loadGroups();
        e.currentTarget.reset();
      }
    } catch (error: any) {
      alert('Error: ' + error.message);
    }
  };

  return (
    <>
      <h2>Groups Management</h2>
      
      <section>
        <h3>Create New Group</h3>
        <form onSubmit={handleCreateGroup}>
          <fieldset>
            <legend>Group Information</legend>
            
            <label htmlFor="name">Group Name:</label>
            <input type="text" id="name" name="name" required />
            <br /><br />
            
            <label htmlFor="description">Description:</label>
            <textarea id="description" name="description" rows={3}></textarea>
            <br /><br />
            
            <button type="submit">Create Group</button>
          </fieldset>
        </form>
      </section>
      
      <hr />
      
      <section>
        <h3>Existing Groups</h3>
        {loading ? (
          <p>Loading groups...</p>
        ) : groups.length > 0 ? (
          <table border={1}>
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Description</th>
              </tr>
            </thead>
            <tbody>
              {groups.map((group) => (
                <tr key={group.id}>
                  <td>{group.id}</td>
                  <td>{group.name}</td>
                  <td>{group.description}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p>No groups found. Create your first group above.</p>
        )}
      </section>
      
      <hr />
      
      <p>
        <a href="/management/dashboard">Back to Dashboard</a>
      </p>
    </>
  );
}
