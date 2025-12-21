'use server';

import axios, { AxiosError } from 'axios';

interface Permission {
  id: string;
  name: string;
  description: string;
  resource?: string;
  action?: string;
}

interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
}

export async function getPermissionsAction(token: string): Promise<ApiResponse<Permission[]>> {
  try {
    const response = await axios.get<Permission[]>('http://localhost:5102/api/permissions', {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    return { success: true, data: response.data };
  } catch (error) {
    if (error instanceof AxiosError) {
      return { 
        success: false, 
        error: error.response?.data?.message || error.message || 'Failed to fetch permissions'
      };
    }
    return { 
      success: false, 
      error: error instanceof Error ? error.message : 'Unknown error occurred' 
    };
  }
}
