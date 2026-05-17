import { format } from "date-fns";

const formatStringToDate = (dateString: string): string => {
  const date = new Date(dateString);
  return format(date, "dd/MMM/yyyy");
};

const formatDateToString = (date: Date | undefined): string => {
  if (!date) return "";
  return format(date, "yyyy-MM-dd");
};

export { formatStringToDate, formatDateToString };
