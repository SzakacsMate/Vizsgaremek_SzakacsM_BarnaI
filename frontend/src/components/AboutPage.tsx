function AboutPage() {
  return (
    <div className="about-page">
      <div className="about-hero">
        <img
          src="/assets/backgrounds/bookAbout.png"
          alt="QuestBook"
          className="about-hero-image"
        />
        <h1 className="about-hero-title">About Us</h1>
      </div>

      <div className="about-cards-section">
        <div className="about-card">
          <h2 className="about-card-title">What is QuestBook?</h2>
          <p className="about-card-text">
            QuestBook is a tabletop role-playing game (TTRPG) community platform
            where players can easily find locations, join sessions, and start new
            adventures. Our goal is to connect RPG enthusiasts and make
            organizing group sessions simpler than ever.
          </p>
        </div>

        <div className="about-card">
          <h2 className="about-card-title">How does it work?</h2>
          <p className="about-card-text">
            Browse registered locations, pick a date, create a lobby, and invite
            your friends — or wait for others to join through the Community tab.
            The system keeps track of your reservations and upcoming sessions.
          </p>
        </div>

        <div className="about-card">
          <h2 className="about-card-title">Creators</h2>
          <p className="about-card-text">
            QuestBook was developed by Szakács M. and Barna I. as an exam
            project. Thank you for using our application!
          </p>
        </div>

        <div className="about-card">
          <h2 className="about-card-title">Want to add a new location?</h2>
          <p className="about-card-text">
            If you'd like to register your venue as a QuestBook location, feel
            free to reach out to us! We'd love to expand our network of
            adventure-friendly spots.
          </p>
          <p className="about-card-text">
            <strong>Email:</strong>{" "}
            <span>contact@questbook.com</span>
          </p>
        </div>
      </div>
    </div>
  );
}

export default AboutPage;
