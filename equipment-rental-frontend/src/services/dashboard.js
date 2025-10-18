import api from './api'

export const dashboardService = {
  async getEquipmentStats() {
    const [total, available, rented, overdue] = await Promise.all([
      api.get('/equipment').then(res => res.data.length),
      api.get('/equipment/available').then(res => res.data.length),
      api.get('/equipment/rented').then(res => res.data.length),
      api.get('/rental/overdue').then(res => res.data.length)
    ])
    
    return { total, available, rented, overdue }
  },

  async getActiveRentals() {
    const response = await api.get('/rental/active')
    return response.data
  },

}
