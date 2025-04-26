import axios from "axios";

const axiosInstance = axios.create({
  baseURL: "http://localhost:5037/api", // buraya kendi backend URL'ini yaz
});

// Request interceptor: Her istek öncesi accessToken'ı ekle
axiosInstance.interceptors.request.use((config) => {
  if (typeof window !== "undefined") {
    const token = localStorage.getItem("accessToken");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
  }
  return config;
}, (error) => {
  return Promise.reject(error);
});

export default axiosInstance;
