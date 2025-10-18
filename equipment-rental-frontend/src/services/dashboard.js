import api from './api'

export const dashboardService = {
  async getEquipmentStats(isAdmin = false) {
    let rentedCountPromise
    if (isAdmin) {
      // Admin sees all rented equipment
      rentedCountPromise = api.get('/equipment/rented').then(res => res.data.length)
    } else {
      // Non-admin users see only their active rentals
      rentedCountPromise = api.get('/rental/active').then(res => res.data.length)
    }

    const [total, available, rented, overdue] = await Promise.all([
      api.get('/equipment').then(res => res.data.length),
      api.get('/equipment/available').then(res => res.data.length),
      rentedCountPromise,
      api.get('/rental/overdue').then(res => res.data.length)
    ])

    return { total, available, rented, overdue }
  },

  async getActiveRentals() {
    const response = await api.get('/rental/active')
    return response.data
  },

}
