import apiClient from './client';
import type { DashboardData, Employee, LeaveActionResponse, LeaveRequest, PagedResult } from '../types';

export const adminApi = {
  getDashboard: async (): Promise<DashboardData> => {
    const response = await apiClient.get('/admin/dashboard');
    return response.data;
  },

  getAllLeaves: async (page = 1, pageSize = 10): Promise<PagedResult<LeaveRequest>> => {
    const response = await apiClient.get(`/admin/leaves?page=${page}&pageSize=${pageSize}`);
    return response.data;
  },

  approveLeave: async (id: string): Promise<LeaveActionResponse> => {
    const response = await apiClient.put(`/admin/approve/${id}`);
    return response.data;
  },

  rejectLeave: async (id: string, comments?: string): Promise<LeaveActionResponse> => {
    const response = await apiClient.put(`/admin/reject/${id}`, { comments });
    return response.data;
  },

  getEmployees: async (page = 1, pageSize = 10): Promise<PagedResult<Employee>> => {
    const response = await apiClient.get(`/admin/employees?page=${page}&pageSize=${pageSize}`);
    return response.data;
  },
};
