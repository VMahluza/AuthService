'use client';

export default function DashboardPage() {
  return (
    <>
      <h2>Dashboard</h2>
      <p>Welcome!</p>
      
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
    </>
  );
}
