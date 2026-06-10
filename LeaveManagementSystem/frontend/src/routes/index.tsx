import { createBrowserRouter, Navigate } from 'react-router-dom';
import AuthLayout from '../layouts/AuthLayout';
import DashboardLayout from '../layouts/DashboardLayout';
import ProtectedRoute from '../components/ProtectedRoute';
import Login from '../pages/auth/Login';
import Register from '../pages/auth/Register';
import EmployeeDashboard from '../pages/employee/Dashboard';
import ApplyLeave from '../pages/employee/ApplyLeave';
import LeaveHistory from '../pages/employee/LeaveHistory';
import Profile from '../pages/employee/Profile';
import AdminDashboard from '../pages/admin/Dashboard';
import AdminLeaveRequests from '../pages/admin/LeaveRequests';
import AdminEmployees from '../pages/admin/Employees';
import AdminReports from '../pages/admin/Reports';

export const router = createBrowserRouter([
  {
    path: '/login',
    element: <AuthLayout />,
    children: [{ index: true, element: <Login /> }],
  },
  {
    path: '/register',
    element: <AuthLayout />,
    children: [{ index: true, element: <Register /> }],
  },
  {
    path: '/',
    element: (
      <ProtectedRoute>
        <DashboardLayout />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <Navigate to="/dashboard" replace /> },
      { path: 'dashboard', element: <EmployeeDashboard /> },
      { path: 'apply-leave', element: <ApplyLeave /> },
      { path: 'leave-history', element: <LeaveHistory /> },
      { path: 'profile', element: <Profile /> },
    ],
  },
  {
    path: '/admin',
    element: (
      <ProtectedRoute requiredRole="Admin">
        <DashboardLayout />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <Navigate to="/admin/dashboard" replace /> },
      { path: 'dashboard', element: <AdminDashboard /> },
      { path: 'leave-requests', element: <AdminLeaveRequests /> },
      { path: 'employees', element: <AdminEmployees /> },
      { path: 'reports', element: <AdminReports /> },
    ],
  },
]);
