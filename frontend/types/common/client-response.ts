export type ClientError = {
  status: number;
  message: string;
};

export type ClientResponse<T> = {
  success: boolean;
  data: T;
  error?: ClientError;
};
