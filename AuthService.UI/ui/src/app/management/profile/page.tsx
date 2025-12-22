'use client';

import { useEffect, useState } from 'react';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Alert from '@mui/material/Alert';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import Avatar from '@mui/material/Avatar';
import Stack from '@mui/material/Stack';
import Divider from '@mui/material/Divider';

interface JwtPayload {
  sub?: string;
  userId?: string;
  userName?: string;
  name?: string;
  email?: string;
  role?: string | string[];
  roles?: string | string[];
  [key: string]: unknown;
}

export default function ProfilePage() {
  const [profile, setProfile] = useState<JwtPayload | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>('');

  const loadProfile = async () => {
    const token = localStorage.getItem('accessToken');
    if (!token) {
      setError('No access token found');
      setLoading(false);
      return;
    }

    try {
      // Decode JWT to display basic info
      const payload = JSON.parse(atob(token.split('.')[1])) as JwtPayload;
      setProfile(payload);
      setError('');
      setLoading(false);
    } catch (error) {
      console.error('Error loading profile:', error);
      setError(error instanceof Error ? error.message : 'Failed to decode token');
      setLoading(false);
    }
  };

  useEffect(() => {
    loadProfile();
  }, []);

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box sx={{ width: '100%', maxWidth: { sm: '100%', md: '1700px' } }}>
      <Typography component="h2" variant="h6" sx={{ mb: 2 }}>
        User Profile
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      {profile ? (
        <Card>
          <CardContent>
            <Grid container spacing={3}>
              <Grid size={{ xs: 12, md: 4 }} sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                <Avatar
                  sx={{ width: 100, height: 100, mb: 2, bgcolor: 'primary.main', fontSize: '3rem' }}
                >
                  {profile.name ? profile.name.charAt(0).toUpperCase() : (profile.userName ? profile.userName.charAt(0).toUpperCase() : 'U')}
                </Avatar>
                <Typography variant="h5">{profile.name || profile.userName || 'Unknown User'}</Typography>
                <Typography variant="body2" color="text.secondary">
                  {profile.email || 'No Email'}
                </Typography>
              </Grid>
              
              <Grid size={{ xs: 12, md: 8 }}>
                <Typography variant="h6" gutterBottom>
                  Details
                </Typography>
                <Divider sx={{ mb: 2 }} />
                
                <Stack spacing={2}>
                  <Box>
                    <Typography variant="subtitle2" color="text.secondary">
                      User ID
                    </Typography>
                    <Typography variant="body1">
                      {profile.sub || profile.userId || 'N/A'}
                    </Typography>
                  </Box>
                  
                  <Box>
                    <Typography variant="subtitle2" color="text.secondary">
                      Username
                    </Typography>
                    <Typography variant="body1">
                      {profile.userName || profile.name || 'N/A'}
                    </Typography>
                  </Box>
                  
                  <Box>
                    <Typography variant="subtitle2" color="text.secondary">
                      Roles
                    </Typography>
                    <Typography variant="body1">
                      {Array.isArray(profile.role) 
                        ? profile.role.join(', ') 
                        : (profile.role || Array.isArray(profile.roles) ? (profile.roles as string[]).join(', ') : (profile.roles as string) || 'None')}
                    </Typography>
                  </Box>
                </Stack>

                <Box sx={{ mt: 4 }}>
                  <Typography variant="h6" gutterBottom>
                    Token Details
                  </Typography>
                  <Divider sx={{ mb: 2 }} />
                  <Box component="pre" sx={{ p: 2, bgcolor: 'grey.100', borderRadius: 1, overflow: 'auto', maxHeight: 300 }}>
                    {JSON.stringify(profile, null, 2)}
                  </Box>
                </Box>
              </Grid>
            </Grid>
          </CardContent>
        </Card>
      ) : (
        <Alert severity="info">No profile information available</Alert>
      )}
    </Box>
  );
}
