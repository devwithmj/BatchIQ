import { useState } from "react";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { X } from "lucide-react";
import { cn } from "@/lib/utils";

export function TagsInput({
  value = [],
  onChange,
  placeholder = "Enter values separated by comma or space",
  className,
}: {
  value: string[];
  onChange: (v: string[]) => void;
  placeholder?: string;
  className?: string;
}) {
  const [input, setInput] = useState("");

  const addTags = () => {
    const tags = input
      .split(/[\s,]+/)
      .map((t) => t.trim())
      .filter((t) => t.length > 0 && !value.includes(t));
    if (tags.length > 0) {
      onChange([...value, ...tags]);
    }
    setInput("");
  };

  const removeTag = (tag: string) => {
    onChange(value.filter((t) => t !== tag));
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (["Enter", " ", ","].includes(e.key)) {
      e.preventDefault();
      addTags();
    }
  };
  return (
    <div className={cn("flex flex-wrap gap-2", className)}>
      {value.map((tag) => (
        <Badge
          key={tag}
          variant="secondary"
          className="flex items-center gap-1"
        >
          {tag}
          <b onClick={() => removeTag(tag)}>X</b>

        </Badge>
      ))}
      <Input
        value={input}
        onChange={(e) => setInput(e.target.value)}
        onKeyDown={handleKeyDown}
        onBlur={addTags}
        placeholder={placeholder}
        className="w-auto flex-1 min-w-[150px]"
      />
    </div>
  );
}
