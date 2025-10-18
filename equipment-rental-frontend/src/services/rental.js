import api from './api'

export const rentalService = {
  async getAll() {
    const response = await api.get('/rentals')
    return response.data
  },

  async getById(id) {
    const response = await api.get(`/rentals/${id}`)
    return response.data
  },

  async getActive() {
    const response = await api.get('/rentals/active')
    return response.data
  },

  async getCompleted() {
    const response = await api.get('/rentals/completed')
    return response.data
  },

  async getOverdue() {
    const response = await api.get('/rentals/overdue')
    return response.data
  },

  async issue(rentalData) {
    const response = await api.post('/rentals/issue', rentalData)
    return response.data
  },

  async return(rentalData) {
    const response = await api.post('/rentals/return', rentalData)
    return response.data
  },

  async extend(id, extensionData) {
    const response = await api.put(`/rentals/${id}`, extensionData)
    return response.data
  },

  async cancel(id) {
    const response = await api.delete(`/rentals/${id}`)
    return response.data
  },

  async getEquipmentHistory(equipmentId) {
    const response = await api.get(`/rentals/equipment/${equipmentId}`)
    return response.data
  }
}
