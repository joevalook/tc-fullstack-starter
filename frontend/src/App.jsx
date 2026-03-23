import { useEffect, useState } from "react";
import { login, register } from "./api/auth";
import { getWorkOrders } from "./api/workOrders";
import { getToken, removeToken, saveToken } from "./utils/auth";

function App() {
  const [mode, setMode] = useState("login");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [token, setToken] = useState(getToken());
  const [workOrders, setWorkOrders] = useState([]);
  const [loading, setLoading] = useState(false);
  const [authLoading, setAuthLoading] = useState(false);
  const [error, setError] = useState("");
  const [authMessage, setAuthMessage] = useState("");

  useEffect(() => {
    const loadWorkOrders = async () => {
      if (!token) {
        setWorkOrders([]);
        return;
      }

      try {
        setLoading(true);
        setError("");
        const data = await getWorkOrders();
        setWorkOrders(data);
      } catch (err) {
        setError(err.message || "Failed to load work orders");
      } finally {
        setLoading(false);
      }
    };

    loadWorkOrders();
  }, [token]);

  const handleSubmit = async (event) => {
    event.preventDefault();

    try {
      setAuthLoading(true);
      setError("");
      setAuthMessage("");

      if (mode === "login") {
        const data = await login(email, password);
        saveToken(data.token);
        setToken(data.token);
        setAuthMessage("Logged in successfully.");
      } else {
        const data = await register(email, password);

        if (data.token) {
          saveToken(data.token);
          setToken(data.token);
          setAuthMessage("Account created and logged in.");
        } else {
          setAuthMessage("Account created. You can now log in.");
          setMode("login");
        }
      }

      setEmail("");
      setPassword("");
    } catch (err) {
      setError(err.message || "Authentication failed");
    } finally {
      setAuthLoading(false);
    }
  };

  const handleLogout = () => {
    removeToken();
    setToken(null);
    setWorkOrders([]);
    setAuthMessage("Logged out.");
    setError("");
  };

  if (!token) {
    return (
      <div className="container">
        <h1>Work Orders App</h1>
        <div className="card">
          <div className="tabs">
            <button
              className={mode === "login" ? "active" : ""}
              onClick={() => {
                setMode("login");
                setError("");
                setAuthMessage("");
              }}
            >
              Login
            </button>
            <button
              className={mode === "signup" ? "active" : ""}
              onClick={() => {
                setMode("signup");
                setError("");
                setAuthMessage("");
              }}
            >
              Sign Up
            </button>
          </div>

          <form onSubmit={handleSubmit} className="form">
            <label>
              Email
              <input
                type="email"
                value={email}
                onChange={(event) => setEmail(event.target.value)}
                required
              />
            </label>

            <label>
              Password
              <input
                type="password"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                required
              />
            </label>

            <button type="submit" disabled={authLoading}>
              {authLoading
                ? "Submitting..."
                : mode === "login"
                ? "Login"
                : "Create Account"}
            </button>
          </form>

          {authMessage && <p className="success">{authMessage}</p>}
          {error && <p className="error">{error}</p>}
        </div>
      </div>
    );
  }

  return (
    <div className="container">
      <div className="header-row">
        <h1>Work Orders</h1>
        <button onClick={handleLogout}>Logout</button>
      </div>

      {authMessage && <p className="success">{authMessage}</p>}
      {error && <p className="error">{error}</p>}

      {loading ? (
        <p>Loading work orders...</p>
      ) : workOrders.length === 0 ? (
        <p>No work orders found.</p>
      ) : (
        <ul className="workorder-list">
          {workOrders.map((workOrder) => (
            <li key={workOrder.id} className="card">
              <h3>{workOrder.title}</h3>
              <p>Status: {workOrder.status}</p>
              {workOrder.description && (
                <p>Description: {workOrder.description}</p>
              )}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

export default App;