import { useEffect, useState } from "react";
import { login, register } from "./api/auth";
import { getWorkOrders } from "./api/workOrders";
import { getToken, removeToken, saveToken } from "./utils/auth";

import AuthForm from "./components/AuthForm";
import WorkOrderList from "./components/WorkOrderList";

function App() {
  const [mode, setMode] = useState("login");
  const [token, setToken] = useState(getToken());
  const [workOrders, setWorkOrders] = useState([]);
  const [loading, setLoading] = useState(false);
  const [authLoading, setAuthLoading] = useState(false);
  const [error, setError] = useState("");
  const [authMessage, setAuthMessage] = useState("");

  useEffect(() => {
    if (!token) return;

    const loadWorkOrders = async () => {
      try {
        setLoading(true);
        const data = await getWorkOrders();
        setWorkOrders(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    loadWorkOrders();
  }, [token]);

  const handleAuthSubmit = async (email, password) => {
    try {
      setAuthLoading(true);
      setError("");
      setAuthMessage("");

      if (mode === "login") {
        const data = await login(email, password);
        saveToken(data.token);
        setToken(data.token);
      } else {
        const data = await register(email, password);

        if (data.token) {
          saveToken(data.token);
          setToken(data.token);
        } else {
          setMode("login");
          setAuthMessage("Account created. Please log in.");
        }
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setAuthLoading(false);
    }
  };

  const handleLogout = () => {
    removeToken();
    setToken(null);
    setWorkOrders([]);
  };

  if (!token) {
    return (
      <div className="container">
        <h1>Work Orders App</h1>

        <AuthForm
          mode={mode}
          onSubmit={handleAuthSubmit}
          loading={authLoading}
          error={error}
          message={authMessage}
          onModeChange={(newMode) => {
            setMode(newMode);
            setError("");
            setAuthMessage("");
          }}
        />
      </div>
    );
  }

  return (
    <div className="container">
      <div className="header-row">
        <h1>Work Orders</h1>
        <button onClick={handleLogout}>Logout</button>
      </div>

      {error && <p className="error">{error}</p>}

      <WorkOrderList workOrders={workOrders} loading={loading} />
    </div>
  );
}

export default App;