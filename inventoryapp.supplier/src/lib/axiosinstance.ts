// src/lib/axiosInstance.ts
import axios from "axios";
import { parseCookies } from "nookies";

const axiosInstance = axios.create({
  baseURL: "http://localhost:5037/api",
  withCredentials: true
});

axiosInstance.interceptors.request.use((config) => {
  const cookies = parseCookies();
  const token = cookies.accessToken;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

export default axiosInstance;
