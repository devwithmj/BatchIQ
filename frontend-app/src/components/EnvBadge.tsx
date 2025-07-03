import { Badge } from "@/components/ui/badge";

export default function EnvBadge() {
  const env = process.env.NEXT_PUBLIC_ENV ?? "dev";
  return (
    <Badge variant="secondary" className="uppercase tracking-wide">
      {env}
    </Badge>
  );
}
