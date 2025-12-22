'use client';

import { useEffect, useState } from 'react';
import { getPermissionsAction } from './actions';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Alert from '@mui/material/Alert';
import CircularProgress from '@mui/material/CircularProgress';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import Link from 'next/link';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';

interface Permission {
  id: string;
  name: string;
  description: string;
  resource?: string;
  action?: string;
}

export default function PermissionsPage() {
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>('');

  const loadPermissions = async () => {
    const token = localStorage.getItem('accessToken');
    if (!token) {
      setError('No access token found');
      setLoading(false);
      return;
    }

    try {
      const result = await getPermissionsAction(token);
      if (result.success && result.data) {
        setPermissions(result.data);
        setError('');
      } else {
        setError(result.error || 'Failed to load permissions');
      }
      setLoading(false);
    } catch (error) {
      console.error('Error loading permissions:', error);
      setError(error instanceof Error ? error.message : 'Unknown error occurred');
      setLoading(false);
    }
  };

  useEffect(() => {
    loadPermissions();
  }, []);

  return (
    <Box sx={{ width: '100%', maxWidth: { sm: '100%', md: '1700px' } }}>
      <Typography component="h2" variant="h6" sx={{ mb: 2 }}>
        Permissions Management
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Card sx={{ mb: 2 }}>
        <CardContent>
          <Typography component="h3" variant="h6" gutterBottom>
            System Permissions
          </Typography>
          {loading ? (
            <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
              <CircularProgress />
            </Box>
          ) : permissions.length > 0 ? (
            <TableContainer component={Paper} elevation={0} variant="outlined">
              <Table aria-label="permissions table">
                <TableHead>
                  <TableRow>
                    <TableCell>ID</TableCell>
                    <TableCell>Name</TableCell>
                    <TableCell>Description</TableCell>
                    <TableCell>Resource</TableCell>
                    <TableCell>Action</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {permissions.map((permission) => (
                    <TableRow key={permission.id}>
                      <TableCell>{permission.id}</TableCell>
                      <TableCell>{permission.name}</TableCell>
                      <TableCell>{permission.description}</TableCell>
                      <TableCell>{permission.resource || 'N/A'}</TableCell>
                      <TableCell>{permission.action || 'N/A'}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          ) : (
            <Typography variant="body1" color="text.secondary">
              No permissions found.
            </Typography>
          )}
        </CardContent>
      </Card>

      <Card>
        <CardContent>
          <Typography component="h3" variant="h6" gutterBottom>
            Permission Assignment
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Use the Roles or Groups pages to assign permissions to roles or groups.
          </Typography>
          <Stack direction="row" spacing={2}>
            <Button component={Link} href="/management/roles" variant="outlined">
              Manage Roles
            </Button>
            <Button component={Link} href="/management/groups" variant="outlined">
              Manage Groups
            </Button>
          </Stack>
        </CardContent>
      </Card>
    </Box>
  );
}
