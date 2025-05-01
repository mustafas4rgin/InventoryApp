'use client';

import Link from 'next/link';

export default function SettingsPage() {
  return (
    <div className="min-h-screen bg-gray-50 dark:bg-[#0f172a] p-6 space-y-8">
      <h1 className="text-3xl font-bold text-gray-900 dark:text-white">⚙️ Ayarlar</h1>
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
        <Link
          href="/settings/profile"
          className="block p-6 bg-white dark:bg-gray-900 rounded-xl shadow hover:shadow-md transition border border-gray-200 dark:border-gray-700"
        >
          <h2 className="text-xl font-semibold text-gray-900 dark:text-white mb-2">👤 Profil Bilgileri</h2>
          <p className="text-gray-600 dark:text-gray-400 text-sm">
            Ad, soyad ve email bilgilerinizi düzenleyin.
          </p>
        </Link>

        <Link
          href="/settings/password"
          className="block p-6 bg-white dark:bg-gray-900 rounded-xl shadow hover:shadow-md transition border border-gray-200 dark:border-gray-700"
        >
          <h2 className="text-xl font-semibold text-gray-900 dark:text-white mb-2">🔐 Şifre Güncelle</h2>
          <p className="text-gray-600 dark:text-gray-400 text-sm">
            Şifrenizi güvenli bir şekilde güncelleyin.
          </p>
        </Link>
      </div>
    </div>
  );
}
