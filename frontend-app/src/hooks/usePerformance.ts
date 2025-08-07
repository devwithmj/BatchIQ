"use client";

import { useEffect, useRef } from "react";

export function usePerformanceMonitor(componentName: string, enabled = false) {
  const renderCount = useRef(0);
  const lastRenderTime = useRef(0);

  useEffect(() => {
    if (!enabled) return;
    
    renderCount.current += 1;
    const now = performance.now();
    
    if (lastRenderTime.current > 0) {
      const timeSinceLastRender = now - lastRenderTime.current;
      
      if (timeSinceLastRender > 16.67) { // > 60fps threshold
        console.warn(
          `⚠️ ${componentName} render took ${timeSinceLastRender.toFixed(2)}ms (render #${renderCount.current})`
        );
      } else {
        console.log(
          `✅ ${componentName} render: ${timeSinceLastRender.toFixed(2)}ms (render #${renderCount.current})`
        );
      }
    }
    
    lastRenderTime.current = now;
  });

  return {
    renderCount: renderCount.current,
    logPerformance: (action: string) => {
      if (enabled) {
        console.log(`📊 ${componentName} - ${action} at ${performance.now().toFixed(2)}ms`);
      }
    }
  };
}

// Custom hook for monitoring search performance
export function useSearchPerformance(enabled = false) {
  const searchTimes = useRef<number[]>([]);

  const startSearch = () => {
    if (!enabled) return () => {};
    
    const startTime = performance.now();
    
    return () => {
      const duration = performance.now() - startTime;
      searchTimes.current.push(duration);
      
      // Keep only last 10 search times
      if (searchTimes.current.length > 10) {
        searchTimes.current.shift();
      }
      
      const avgTime = searchTimes.current.reduce((a, b) => a + b, 0) / searchTimes.current.length;
      
      if (duration > 100) {
        console.warn(`🔍 Slow search: ${duration.toFixed(2)}ms (avg: ${avgTime.toFixed(2)}ms)`);
      } else {
        console.log(`🔍 Search: ${duration.toFixed(2)}ms (avg: ${avgTime.toFixed(2)}ms)`);
      }
    };
  };

  return { startSearch };
}
