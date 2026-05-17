import TopProductsBoard from "@/components/TopProductsBoard";
import Image from "next/image";

export default function Home() {
  return (
    <div className="flex flex-col flex-1 items-center justify-center bg-zinc-50 font-sans ">
      <main className="flex flex-1 w-full max-w-3xl flex-col items-center gap-8 py-10 sm:py-32 px-6 sm:px-16 bg-white  sm:items-start">
        <div className="relative w-full h-80">
          <Image src="/logo.png" alt="Bunnings Logo" fill objectFit="contain" />
        </div>

        <TopProductsBoard />
      </main>
    </div>
  );
}
