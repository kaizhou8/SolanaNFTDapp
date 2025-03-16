import React, { useState } from 'react';

interface NFTFormData {
  name: string;
  description: string;
  image: string;
}

interface CreateNFTFormProps {
  onSubmit: (data: NFTFormData) => void;
  onCancel: () => void;
}

const CreateNFTForm: React.FC<CreateNFTFormProps> = ({ onSubmit, onCancel }) => {
  const [formData, setFormData] = useState<NFTFormData>({
    name: '',
    description: '',
    image: 'https://via.placeholder.com/300', // Default placeholder image
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value,
    });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(formData);
  };

  return (
    <div className="card" style={{ marginBottom: '20px' }}>
      <h3>Create New NFT</h3>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="name" className="form-label">Name</label>
          <input
            type="text"
            id="name"
            name="name"
            className="form-control"
            value={formData.name}
            onChange={handleChange}
            required
          />
        </div>
        
        <div className="form-group">
          <label htmlFor="description" className="form-label">Description</label>
          <textarea
            id="description"
            name="description"
            className="form-control"
            value={formData.description}
            onChange={handleChange}
            rows={3}
            required
          />
        </div>
        
        <div className="form-group">
          <label htmlFor="image" className="form-label">Image URL</label>
          <input
            type="url"
            id="image"
            name="image"
            className="form-control"
            value={formData.image}
            onChange={handleChange}
            required
          />
        </div>
        
        <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '10px', marginTop: '20px' }}>
          <button type="button" className="btn btn-secondary" onClick={onCancel}>
            Cancel
          </button>
          <button type="submit" className="btn">
            Create NFT
          </button>
        </div>
      </form>
    </div>
  );
};

export default CreateNFTForm; 