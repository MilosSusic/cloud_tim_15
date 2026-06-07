import React, { createContext, useContext, useEffect, useState } from "react";
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";
import { useAuth } from "../auth/AuthContext";
import { CONFIG } from "../config/config"; //

interface SignalRContextType {
  connection: HubConnection | null;
  isConnected: boolean;
}

const SignalRContext = createContext<SignalRContextType | undefined>(undefined);

export const SignalRProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [connection, setConnection] = useState<HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  const { token, isAuthenticated } = useAuth();

  useEffect(() => {
    // Konekciju pravimo samo ako je korisnik autentifikovan i imamo token
    if (!isAuthenticated || !token) {
      if (connection) {
        connection.stop();
        setConnection(null);
        setIsConnected(false);
      }
      return;
    }

    // Konfiguracija SignalR Hub-a
    // Prilagodi URL ka tvom backend SmartScaleHub-u
    const newConnection = new HubConnectionBuilder()
      .withUrl(CONFIG.SIGNALR_HUB_URL, {
        accessTokenFactory: () => token, // Prosleđujemo JWT token automatski
      })
      .withAutomaticReconnect() // Automatski pokušava ponovno povezivanje ako pukne net
      .configureLogging(LogLevel.Information)
      .build();

    newConnection
      .start()
      .then(() => {
        console.log(
          "SignalR Connected successfully to:",
          CONFIG.SIGNALR_HUB_URL,
        );
        setIsConnected(true);
        setConnection(newConnection);
      })
      .catch((err) => console.error("SignalR Connection Error: ", err));

    // Cleanup prilikom unmount-ovanja ili odjave korisnika
    return () => {
      newConnection.stop();
    };
  }, [isAuthenticated, token]);

  return (
    <SignalRContext.Provider value={{ connection, isConnected }}>
      {children}
    </SignalRContext.Provider>
  );
};

export const useSignalR = () => {
  const context = useContext(SignalRContext);
  if (!context) {
    throw new Error("useSignalR must be used within a SignalRProvider");
  }
  return context;
};
