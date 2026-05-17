import { ClientError, ClientResponse } from "@/types/common/client-response";
import axios, { AxiosRequestConfig } from "axios";

const service = axios.create({
  baseURL: process.env.NEXT_PUBLIC_WEB_HOST_API_BASE_URL,
  timeout: 5000,
  headers: {
    "Content-Type": "application/json",
  },
});

service.interceptors.response.use(
  (response) => response,
  (error) => Promise.reject(error.response?.data?.error as ClientError),
);

const request = {
  async get<T>(
    url: string,
    params?: object,
    config?: AxiosRequestConfig,
  ): Promise<ClientResponse<T>> {
    try {
      const response = await service.get<ClientResponse<T>>(url, {
        params,
        ...config,
      });

      return response.data;
    } catch (error) {
      const clientError = error as ClientError;
      return {
        success: false,
        body: null as T,
        error: clientError,
      };
    }
  },
  // You can add other HTTP methods (post, put, delete, etc.) similarly
};

export default request;
