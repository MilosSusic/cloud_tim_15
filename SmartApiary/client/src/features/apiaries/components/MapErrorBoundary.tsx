import React from "react";

interface Props { children: React.ReactNode; }
interface State { hasError: boolean; }

class MapErrorBoundary extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = { hasError: false };
  }
  static getDerivedStateFromError() { return { hasError: true }; }
  componentDidCatch(error: unknown, info: unknown) { console.error("Map error:", error, info); }
  render() {
    if (this.state.hasError) return <div style={{ color: "red" }}>Greška u prikazu mape.</div>;
    return this.props.children;
  }
}

export default MapErrorBoundary;
