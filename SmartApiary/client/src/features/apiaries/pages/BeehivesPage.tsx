import React, { useEffect, useState } from 'react';
import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import { getApiaries, createApiary } from '../api/apiaryApi';

import 'leaflet/dist/leaflet.css';
import '../layout/BeehivesPage.css';
import type { Apiary } from '../types/Apiary';

const BeehivesPage: React.FC = () => {
  const [apiaries, setApiaries] = useState<Apiary[]>([]);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({
    name: '',
    description: '',
    latitude: '',
    longitude: '',
    file: undefined as File | undefined,
  });
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  // const navigate = useNavigate();

  useEffect(() => {
    fetchApiaries();
  }, []);

  const fetchApiaries = async () => {
    setLoading(true);
    try {
      const data = await getApiaries();
      setApiaries(data);
      setMessage('Successfully fetched apiaries.');
    } catch {
      setMessage('Failed to fetch apiaries.');
    }
    setLoading(false);
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value, files } = e.target as any;
    if (name === 'file') {
      setForm({ ...form, file: files[0] });
    } else {
      setForm({ ...form, [name]: value });
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      await createApiary(form);
      setShowForm(false);
      setForm({ name: '', description: '', latitude: '', longitude: '', file: undefined });
      setMessage('Apiary created successfully.');
      await fetchApiaries();
    } catch (err) {
      setMessage('Failed to create apiary.');
    }
    setLoading(false);
  };

  return (
    <div className="beehives-page">
      <h2>My Apiaries</h2>
      <button onClick={() => setShowForm(!showForm)} className="add-btn">+ Add Apiary</button>
      {message && <div style={{margin: '16px 0', color: message.includes('success') ? '#388e3c' : '#d32f2f', fontWeight: 600, background: '#fffde7', border: '1px solid #ffe082', borderRadius: 8, padding: 10, maxWidth: 420}}>{message}</div>}
      {showForm && (
        <form className="apiary-form" onSubmit={handleSubmit}>
          <div className="apiary-form-grid">
            <div className="apiary-form-group">
              <label htmlFor="apiary-name">Apiary name</label>
              <input id="apiary-name" name="name" placeholder="Apiary name" value={form.name} onChange={handleInputChange} required />
            </div>
            <div className="apiary-form-group">
              <label htmlFor="apiary-description">Description</label>
              <textarea id="apiary-description" name="description" placeholder="Description" value={form.description} onChange={handleInputChange} />
            </div>
            <div className="apiary-form-group">
              <label htmlFor="apiary-latitude">Latitude</label>
              <input id="apiary-latitude" name="latitude" placeholder="Latitude" value={form.latitude} onChange={handleInputChange} required />
            </div>
            <div className="apiary-form-group">
              <label htmlFor="apiary-longitude">Longitude</label>
              <input id="apiary-longitude" name="longitude" placeholder="Longitude" value={form.longitude} onChange={handleInputChange} required />
            </div>
            <div className="apiary-form-group apiary-form-file-group">
              <label htmlFor="apiary-file">Image</label>
              <input id="apiary-file" name="file" type="file" accept="image/*" onChange={handleInputChange} required />
            </div>
          </div>
          <button type="submit" disabled={loading}>Save</button>
        </form>
      )}
      <div style={{display: 'flex', gap: 32, alignItems: 'flex-start', justifyContent: 'center', flexWrap: 'wrap'}}>
        <div style={{minWidth: 260, maxWidth: 320, background: '#fffde7', border: '1px solid #ffe082', borderRadius: 14, padding: 18, boxShadow: '0 2px 12px rgba(255,183,0,0.08)', marginBottom: 24}}>
          <h3 style={{color: '#e65100', fontSize: '1.2rem', marginBottom: 12}}>Apiary List</h3>
          {apiaries.length === 0 && <div style={{color: '#888'}}>No apiaries found.</div>}
          <ul style={{listStyle: 'none', padding: 0, margin: 0}}>
            {apiaries.map(apiary => (
              <li key={apiary.id} style={{marginBottom: 18, borderBottom: '1px solid #ffe082', paddingBottom: 10}}>
                <div style={{fontWeight: 600, color: '#e65100'}}>{apiary.name}</div>
                <div style={{fontSize: '0.97rem', color: '#444'}}>{apiary.description}</div>
                <div style={{fontSize: '0.93rem', color: '#888'}}>Lat: {apiary.latitude}, Lng: {apiary.longitude}</div>
              </li>
            ))}
          </ul>
        </div>
        <div className="map-section" style={{flex: 1, minWidth: 340}}>
          <MapContainer center={[45.25, 19.85]} zoom={8} style={{ height: '400px', width: '100%' }}>
            <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
            {apiaries.map(apiary => (
              <Marker key={apiary.id} position={[apiary.latitude, apiary.longitude]}>
                <Popup>
                  <div>
                    {/* Prikazuje thumbnailUrl ako postoji, u suprotnom koristi imageUrl */}
                    <img
                      src={apiary.thumbnailUrl || apiary.imageUrl}
                      alt={apiary.name}
                      style={{ width: 100 }}
                    />
                    <h4>{apiary.name}</h4>
                    <p>{apiary.description}</p>
                  </div>
                </Popup>
              </Marker>
            ))}
          </MapContainer>
        </div>
      </div>
    </div>
  );
};

export default BeehivesPage;
