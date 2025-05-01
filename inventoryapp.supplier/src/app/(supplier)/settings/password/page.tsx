'use client';

import { useState } from 'react';
import axiosInstance from '@/lib/axiosinstance';
import toast from 'react-hot-toast';

export default function PasswordSettings() {
  const [oldPassword, setOldPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [loading, setLoading] = useState(false);

  const extractErrorMessage = (err: any) => {
    if (Array.isArray(err.response?.data?.errors)) {
      return err.response.data.errors.map((e: any) => e.errorMessage).join(' | ');
    }
    if (typeof err.response?.data?.message === 'string') {
      return err.response.data.message;
    }
    return 'İşlem başarısız oldu';
  };

  const handleResetPassword = async (e: React.FormEvent) => {
    e.preventDefault();
    if (newPassword !== confirmPassword) {
      toast.error('Yeni şifreler eşleşmiyor');
      return;
    }

    try {
      setLoading(true);
      const res = await axiosInstance.put(`/auth/reset-password?oldPassword=${oldPassword}&newPassword=${newPassword}`);
      const message = res.data?.message || res.data;
      toast.success(message);
      setOldPassword('');
      setNewPassword('');
      setConfirmPassword('');
    } catch (err: any) {
      toast.error(extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-[#0f172a] flex items-center justify-center px-4">
      <div className="w-full max-w-lg bg-white dark:bg-gray-900 rounded-2xl shadow-xl p-8">
        <h1 className="text-3xl font-bold text-center text-gray-900 dark:text-white mb-6">🔐 Şifre Güncelle</h1>
        <form onSubmit={handleResetPassword} className="space-y-5">
          <input
            type="password"
            value={oldPassword}
            onChange={(e) => setOldPassword(e.target.value)}
            placeholder="Mevcut şifreniz"
            required
            className="w-full px-4 py-2 border rounded-lg bg-white dark:bg-gray-800 dark:border-gray-700 dark:text-white shadow-sm"
          />
          <input
            type="password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            placeholder="Yeni şifre"
            required
            className="w-full px-4 py-2 border rounded-lg bg-white dark:bg-gray-800 dark:border-gray-700 dark:text-white shadow-sm"
          />
          <input
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            placeholder="Yeni şifre (tekrar)"
            required
            className="w-full px-4 py-2 border rounded-lg bg-white dark:bg-gray-800 dark:border-gray-700 dark:text-white shadow-sm"
          />
          <button
            type="submit"
            disabled={loading}
            className="w-full py-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg transition shadow-md"
          >
            {loading ? 'Güncelleniyor...' : 'Şifreyi Güncelle'}
          </button>
        </form>
      </div>
    </div>
  );
}
