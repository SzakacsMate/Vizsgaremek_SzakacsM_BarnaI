import "./App.css";
import LocationCard from "./components/LocationCard";
import UpcomingSessionCard from "./components/UpcomingSessionCard";
import { currentUser, locations, upcomingSessions } from "./data/mockData";

import questBookLogo from "./assets/branding/QuestBook.png";
import bellIcon from "./assets/icons/bell.png";
import magnifierIcon from "./assets/icons/magnifier.png";

function App() {
  return (
    <div className="page">
      <div className="app-shell">
        <header className="navbar">
          <div className="navbar-logo-section">
            <img
              src={questBookLogo}
              alt="QuestBook"
              className="navbar-logo"
            />
          </div>

          <div className="navbar-divider" />

          <nav className="navbar-links">
            <a href="/">Locations</a>
            <a href="/">My Sessions</a>
            <a href="/">Community</a>
          </nav>

          <div className="navbar-divider" />

          <div className="navbar-icons">
            <img src={bellIcon} alt="Notifications" />
            <img src={magnifierIcon} alt="Search" />
          </div>
        </header>

        <div className="content-layout">
          <main className="main-content">
            <div className="main-overlay">
              <h1 className="page-title">Locations</h1>

              <section className="locations-list">
                {locations.map((location) => (
                  <LocationCard key={location.id} location={location} />
                ))}
              </section>
            </div>
          </main>

          <aside className="sidebar">
            <div className="sidebar-overlay">
              {currentUser ? (
                <>
                  <h2 className="sidebar-title">
                    Welcome back, {currentUser.name}!
                  </h2>

                  <img
                    src={currentUser.image}
                    alt={currentUser.name}
                    className="sidebar-user-image"
                  />
                </>
              ) : (
                <h2 className="sidebar-title">
                  You are currently not logged in.
                </h2>
              )}

              <div className="sidebar-sessions">
                <h3 className="sidebar-subtitle">Upcoming Reservations</h3>

                {upcomingSessions.slice(0, 2).map((session) => (
                  <UpcomingSessionCard key={session.id} session={session} />
                ))}
              </div>
            </div>
          </aside>
        </div>
      </div>
    </div>
  );
}

export default App;