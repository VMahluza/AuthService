import axios, { AxiosInstance } from 'axios';
import { BACKEND_BASE_URL } from '../constants';

/**
 * Create and configure axios instance
 * Single Responsibility: Axios configuration only
 */
export const apiClient: AxiosInstance = axios.create({
  baseURL: BACKEND_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000, // 10 seconds
});
