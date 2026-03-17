import loginBackground from "../assets/auth/LoginBackground.jpg";
import registerBackground from "../assets/auth/RegisterBackground.jpg";
import redCover from "../assets/auth/RedSeeThroughCover.png";

type AuthMode = "login" | "register";

type AuthPageProps = {
  mode: AuthMode;
  onSwitchMode: (mode: AuthMode) => void;
  onSubmit: (data: {
    mode: AuthMode;
    username: string;
    email: string;
    password: string;
  }) => Promise<void>;
};

export default function AuthPage({
  mode,
  onSwitchMode,
  onSubmit,
}: AuthPageProps) {
  const isLogin = mode === "login";
  const backgroundImage = isLogin ? loginBackground : registerBackground;
  const title = isLogin ? "LOGIN" : "REGISTER";

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const formData = new FormData(event.currentTarget);

    const username = String(formData.get("username") ?? "");
    const email = String(formData.get("email") ?? "");
    const password = String(formData.get("password") ?? "");

    await onSubmit({
      mode,
      username,
      email,
      password,
    });
  };

  return (
    <div className="auth-page">
      <div className={`auth-shell ${isLogin ? "auth-login" : "auth-register"}`}>
        <div
          className="auth-visual"
          style={{ backgroundImage: `url(${backgroundImage})` }}
        >
          <img src={redCover} alt="" className="auth-red-cover" />
        </div>

        <div className="auth-panel">
          <form className="auth-form" onSubmit={handleSubmit}>
            <h1 className="auth-title">{title}</h1>

            <input
              className="auth-input"
              type="text"
              name="username"
              placeholder="Username"
              required
            />

            <input
              className="auth-input"
              type="email"
              name="email"
              placeholder="Email Address"
              required
            />

            <input
              className="auth-input"
              type="password"
              name="password"
              placeholder="Password"
              required
            />

            {!isLogin && (
              <input
                className="auth-input"
                type="password"
                name="passwordAgain"
                placeholder="Password Again"
                required
              />
            )}

            <button className="auth-submit-button" type="submit">
              {isLogin ? "Login Now" : "Register Now"}
            </button>

            <button
              type="button"
              className="auth-switch-button"
              onClick={() => onSwitchMode(isLogin ? "register" : "login")}
            >
              {isLogin
                ? "No account yet? Register"
                : "Already have an account? Login"}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}