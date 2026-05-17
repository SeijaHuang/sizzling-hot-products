"use client";

import getHotProductsAsync from "@/services/get-hot-products";
import { SERVICE_KEYS } from "@/types/common/service-keys";
import { useQuery } from "@tanstack/react-query";
import { Skeleton } from "@/components/ui/skeleton";
import { DatePicker } from "../Shared/DatePicker";
import { useState } from "react";
import { formatDateToString } from "@/utils/format-date";
import HotProductsTable from "./components/TopProductsTable";
import HotProductsHeader from "./components/TopProductsHeader";

const id = "top-products-board";

const TopProductsBoard = () => {
  const [startDate, setStartDate] = useState<Date | undefined>(
    new Date("2026-04-21"),
  );
  const [endDate, setEndDate] = useState<Date | undefined>(
    new Date("2026-04-23"),
  );

  const { isLoading, data } = useQuery({
    queryKey: [SERVICE_KEYS.GET_HOT_PRODUCTS, startDate, endDate],
    queryFn: () =>
      getHotProductsAsync({
        startDate: formatDateToString(startDate),
        endDate: formatDateToString(endDate),
      }),
    select: (response) => response.body,
    enabled: !!startDate,
  });

  const renderContent = (): React.ReactNode => {
    if (isLoading)
      return (
        <Skeleton
          id={`${id}-skeleton`}
          data-testid={`${id}-skeleton`}
          className="w-full h-48"
        />
      );

    if (!data?.daily.length)
      return (
        <div
          id={`${id}-no-results`}
          data-testid={`${id}-no-results`}
          className="text-center text-xl"
        >
          No hot products found.
        </div>
      );

    return <HotProductsTable id={id} daily={data.daily} period={data.period} />;
  };

  return (
    <div className="w-full text-neutral-800 flex flex-col items-center gap-6">
      <HotProductsHeader />
      <div className="flex items-center gap-1 sm:gap-4 ">
        <DatePicker
          id={`${id}-start-date`}
          value={startDate}
          onChange={setStartDate}
          disabled={(date) => (endDate ? date > endDate : false)}
        />
        <span className="text-secondary">—</span>
        <DatePicker
          id={`${id}-end-date`}
          value={endDate}
          onChange={setEndDate}
          disabled={(date) => (startDate ? date < startDate : false)}
        />
      </div>
      {renderContent()}
    </div>
  );
};

export default TopProductsBoard;
