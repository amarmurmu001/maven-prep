export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: 'Admin' | 'Employee';
}

export interface AuthResponse {
  success: boolean;
  message: string;
  accessToken?: string;
  refreshToken?: string;
  expiresAt?: string;
  user?: User;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface LeaveRequest {
  id: string;
  employeeId: string;
  employeeName: string;
  leaveType: string;
  startDate: string;
  endDate: string;
  durationDays: number;
  reason: string;
  status: 'Pending' | 'Approved' | 'Rejected' | 'Cancelled';
  createdAt: string;
  updatedAt?: string;
  reviewedByName?: string;
  comments?: string;
}

export interface ApplyLeaveRequest {
  leaveType: string;
  startDate: string;
  endDate: string;
  reason: string;
}

export interface LeaveActionResponse {
  success: boolean;
  message: string;
  leaveRequest?: LeaveRequest;
}

export interface DashboardData {
  totalEmployees: number;
  activeEmployees: number;
  pendingRequests: number;
  approvedRequests: number;
  rejectedRequests: number;
  totalLeavesThisMonth: number;
  leaveTypeDistribution: Record<string, number>;
  recentRequests: RecentLeave[];
}

export interface RecentLeave {
  id: string;
  employeeName: string;
  leaveType: string;
  startDate: string;
  endDate: string;
  status: string;
  createdAt: string;
}

export interface Employee {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  isActive: boolean;
  totalLeaves: number;
  pendingLeaves: number;
  approvedLeaves: number;
  createdAt: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ApiError {
  success: boolean;
  message: string;
  statusCode: number;
  errors?: Record<string, string[]>;
}
