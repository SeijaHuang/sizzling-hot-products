import FlameIcon from "../../Shared/FlameIcon";

const HotProductsHeader = () => {
  return (
    <div className="flex items-center gap-4">
      <FlameIcon />
      <h1 className="text-lg sm:text-2xl font-bold text-center">
        Top Sizzling Hot Products
      </h1>
      <FlameIcon />
    </div>
  );
};

export default HotProductsHeader;
