import request from "@/lib/request";
import { ClientResponse } from "@/types/common/client-response";
import {
  GetTopProductsRequest,
  GetTopProductsResponse,
} from "@/types/services/get-top-products";

const getHotProductsAsync = async ({
  startDate,
  endDate,
}: GetTopProductsRequest): Promise<ClientResponse<GetTopProductsResponse>> => {
  const response: ClientResponse<GetTopProductsResponse> =
    await request.get<GetTopProductsResponse>("/products", {
      startDate,
      endDate,
    });
  return response;
};

export default getHotProductsAsync;
