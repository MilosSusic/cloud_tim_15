import React from "react";

interface ApiaryFormValues {
  name: string;
  description: string;
  latitude: string;
  longitude: string;
  file?: File;
}

interface ApiaryFormErrors {
  name?: string;
  description?: string;
  latitude?: string;
  longitude?: string;
}

interface Props {
  title: string;
  submitLabel: string;
  form: ApiaryFormValues;
  imagePreview: string | null;
  existingImageUrl?: string | null;
  loading: boolean;
  errors: ApiaryFormErrors;
  shouldShowError: (field: keyof ApiaryFormErrors) => boolean;
  onChange: (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => void;
  onSubmit: (e: React.FormEvent) => void;
  onClose: () => void;
  imageRequired?: boolean;
}

const ApiaryForm: React.FC<Props> = ({
  title, submitLabel, form, imagePreview, existingImageUrl, loading,
  errors, shouldShowError, onChange, onSubmit, onClose, imageRequired = false,
}) => (
  <div className="apiary-modal-backdrop" onClick={onClose}>
    <form className="apiary-form" onSubmit={onSubmit} onClick={(e) => e.stopPropagation()}>
      <div className="apiary-form-header">
        <h3 className="apiary-form-title">{title}</h3>
        <button type="button" className="apiary-info-close" onClick={onClose} aria-label="Close form">×</button>
      </div>
      <div className="apiary-form-grid">
        <div className="apiary-form-group">
          <label htmlFor="apiary-name">Name</label>
          <input id="apiary-name" name="name" placeholder="Apiary name" value={form.name} onChange={onChange} />
          {errors.name && shouldShowError("name") && <span className="apiary-form-error">{errors.name}</span>}
        </div>
        <div className="apiary-form-group">
          <label htmlFor="apiary-description">Description</label>
          <textarea id="apiary-description" name="description" placeholder="Description" value={form.description} onChange={onChange} />
          {errors.description && shouldShowError("description") && <span className="apiary-form-error">{errors.description}</span>}
        </div>
        <div className="apiary-form-group">
          <label htmlFor="apiary-latitude">Latitude</label>
          <input id="apiary-latitude" name="latitude" type="number" min={-90} max={90} step="any" placeholder="Latitude" value={form.latitude} onChange={onChange} />
          {errors.latitude && shouldShowError("latitude") && <span className="apiary-form-error">{errors.latitude}</span>}
        </div>
        <div className="apiary-form-group">
          <label htmlFor="apiary-longitude">Longitude</label>
          <input id="apiary-longitude" name="longitude" type="number" min={-180} max={180} step="any" placeholder="Longitude" value={form.longitude} onChange={onChange} />
          {errors.longitude && shouldShowError("longitude") && <span className="apiary-form-error">{errors.longitude}</span>}
        </div>
        <div className="apiary-form-group apiary-form-file-group">
          <label htmlFor="apiary-file">{imageRequired ? "Image" : "Image (optional)"}</label>
          <input id="apiary-file" name="file" type="file" accept="image/*" onChange={onChange} />
          <label htmlFor="apiary-file" className="apiary-form-file-label">
            {imageRequired ? "Choose Image" : "Choose New Image"}
          </label>
          {(imagePreview || existingImageUrl) && (
            <img src={imagePreview || existingImageUrl!} alt="Preview" className="apiary-form-image-preview" />
          )}
        </div>
      </div>
      <button type="submit" disabled={loading}>
        {loading ? "Saving..." : submitLabel}
      </button>
    </form>
  </div>
);

export default ApiaryForm;
