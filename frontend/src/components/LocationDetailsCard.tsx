import type { Location } from "../types/location";

type LocationDetailsCardProps = {
  location: Location;
  onBack: () => void;
};

export default function LocationDetailsCard({
  location,
  onBack,
}: LocationDetailsCardProps) {
  return (
    <section className="location-details-card">
      <div className="location-details-top">
        <img
          src={location.image}
          alt={location.name}
          className="location-details-image"
        />

        <div className="location-details-header-block">
          <div className="location-details-title-box">
            <h2 className="location-details-title">{location.name}</h2>
            <p className="location-details-address">{location.address}</p>
          </div>
        </div>
      </div>

      <div className="location-details-description">
        <p>{location.longDescription}</p>
      </div>

      <div className="location-details-actions">
        <button className="location-details-back-button" onClick={onBack}>
          BACK
        </button>

        <button
          className="location-details-reserve-button"
          onClick={() => alert(`Reserve clicked for: ${location.name}`)}
        >
          RESERVE
        </button>
      </div>
    </section>
  );
}