# Start from Ubuntu 24.04
FROM clockworklabs/spacetime

USER root

RUN apt-get update && apt-get install sudo && echo "spacetime  ALL=(ALL) NOPASSWD:ALL" | sudo tee /etc/sudoers.d/spacetime

USER spacetime

RUN mkdir -p /home/spacetime/.config/spacetime/
RUN mkdir -p /home/spacetime/.local/share/spacetime/data
