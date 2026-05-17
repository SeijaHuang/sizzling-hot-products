export type ClientError = {
  status: number;
  message: string;
};

export type ClientResponse<T> = {
  success: boolean;
  body: T;
  error?: ClientError;
};
